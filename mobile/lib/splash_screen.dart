import 'package:flutter/material.dart';

import 'login_page.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen>
    with SingleTickerProviderStateMixin {
  // Controls the wipe animation. 0.0 = fully hidden, 1.0 = fully visible.
  late AnimationController _controller;
  late Animation<double> _reveal;

  @override
  void initState() {
    super.initState();

    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800), // how long ONE wipe takes
    );

    _reveal = Tween<double>(begin: 0.0, end: 1.0).animate(_controller);

    _playSplashSequence();
  }

  // Runs the 3 steps one after another, just like awaiting promises in JS.
  Future<void> _playSplashSequence() async {
    await _controller.forward(); // Step 1: wipe in (0 -> 1), 800ms
    await Future.delayed(const Duration(milliseconds: 1400)); // Step 2: hold
    await _controller.reverse(); // Step 3: wipe out (1 -> 0), 800ms

    // Total time: 800 + 1400 + 800 = 3000ms (3 seconds), guaranteed.

    if (mounted) {
      Navigator.of(context).pushReplacement(
        MaterialPageRoute(builder: (context) => const LoginPage()),
      );
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: AnimatedBuilder(
          animation: _reveal,
          builder: (context, child) {
            return ClipRect(clipper: _WipeClipper(_reveal.value), child: child);
          },
          child: Image.asset(
            'assets/images/ibnsina-pharma-logo.png',
            width: 200,
            fit: BoxFit.contain,
          ),
        ),
      ),
    );
  }
}

// Only shows the left portion of its child, based on [fraction] (0.0 to 1.0).
// The box size never changes, so the image stays perfectly centered —
// only the visible "window" grows/shrinks from the left edge.
class _WipeClipper extends CustomClipper<Rect> {
  final double fraction;

  _WipeClipper(this.fraction);

  @override
  Rect getClip(Size size) {
    return Rect.fromLTRB(0, 0, size.width * fraction, size.height);
  }

  @override
  bool shouldReclip(_WipeClipper oldClipper) {
    return oldClipper.fraction != fraction;
  }
}
