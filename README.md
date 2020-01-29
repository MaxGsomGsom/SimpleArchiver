[![Build Status](https://travis-ci.org/MaxGsomGsom/SimpleArchiver.svg?branch=master)](https://travis-ci.org/MaxGsomGsom/SimpleArchiver)

# Compressed file structure

| Offset | Data 
| ---|---
| 0 | Length of input file
| 8 | Size of input block
| 12 | Block 0: length of compressed block
| 16 | Block 0: compressed data
| x | Block 1: length of compressed block
| x+4 | Block 1: compressed data
