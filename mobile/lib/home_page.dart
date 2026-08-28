import 'package:flutter/material.dart';

import 'image_carousel.dart';

class HomePage extends StatelessWidget {
  final String name;

  const HomePage({super.key, required this.name});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Column(
          children: [
            Container(
              color: const Color.fromARGB(255, 18, 107, 180),
              padding: const EdgeInsets.all(16),
              width: double.infinity,
              child: Text(
                'Welcome $name',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(height: 20),
            const ImageCarousel(),
          ],
        ),
      ),
    );
  }
}
